package main

import (
	"database/sql"
	"encoding/json"
	"fmt"
	"log"
	"net/http"

	"github.com/go-chi/chi/v5"
	"github.com/go-chi/chi/v5/middleware"
	"github.com/google/uuid"
	_ "github.com/lib/pq"
)

type Player struct {
	Id              string `json:"id"`
	Nickname        string `json:"nickname"`
	Rank            Rank   `json:"rank"`
	Credits         int    `json:"credits"`
	Experience      int    `json:"experience"`
	CommandCenterId string `json:"commandCenterId"`
	IsDeleted       bool   `json:"isDeleted"`
}

type CommandCenter struct {
	Id                         string     `json:"id"`
	PlayerId                   string     `json:"playerId"`
	MissionId                  string     `json:"missionId"`
	HighestDifficultyAvailable Difficulty `json:"HighestDifficultyAvailable"`
	IsDeleted                  bool       `json:"isDeleted"`
}

type Difficulty int

const (
	Easy Difficulty = 1
	Medium
	Hard
	Expert
	Hell
)

type Rank string

const (
	Cadet     Rank = "Cadet"
	Soldier   Rank = "Soldier"
	Captain   Rank = "Captain"
	Commander Rank = "Commander"
)

var db *sql.DB

func main() {
	var err error
	db, err = sql.Open("postgres", "host=localhost port=5433 user=postgres password=postgres dbname=helldivers sslmode=disable")

	if err != nil {
		log.Fatal(err)
	}
	defer db.Close()

	r := chi.NewRouter()

	r.Use(middleware.Logger)
	r.Use(middleware.Recoverer)

	r.Get("/players", getPlayers)
	r.Get("/players/{id}", getPlayer)
	r.Post("/players", createPlayer)

	fmt.Println("Server started on :8080")
	http.ListenAndServe(":8080", r)
}

func getPlayer(w http.ResponseWriter, r *http.Request) {
	id := chi.URLParam(r, "id")
	var player Player
	err := db.QueryRow("SELECT \"Id\", \"Nickname\", \"Rank\", \"Credits\", \"Experience\", \"CommandCenterId\", \"IsDeleted\" FROM \"Players\" WHERE \"Id\" = $1", id).Scan(&player.Id, &player.Nickname, &player.Rank, &player.Credits, &player.Experience, &player.CommandCenterId, &player.IsDeleted)

	if err != nil {
		fmt.Println(err)
		return
	}

	w.Header().Set("Content-Type", "application/json")
	json.NewEncoder(w).Encode(player)
}

func getPlayers(w http.ResponseWriter, r *http.Request) {
	var players []Player

	rows, err := db.Query("SELECT \"Id\", \"Nickname\", \"Rank\", \"Credits\", \"Experience\", \"CommandCenterId\", \"IsDeleted\" FROM \"Players\"")

	if err != nil {
		fmt.Println(err)
		return
	}

	for rows.Next() {
		p := Player{}
		err := rows.Scan(&p.Id, &p.Nickname, &p.Rank, &p.Credits, &p.Experience, &p.CommandCenterId, &p.IsDeleted)
		if err != nil {
			fmt.Println(err)
			continue
		}
		players = append(players, p)
	}

	w.Header().Set("Content-Type", "application/json")
	json.NewEncoder(w).Encode(players)
}

func createPlayer(w http.ResponseWriter, r *http.Request) {
	type CreatePlayerRequest struct {
		Nickname string `json:"nickname"`
	}
	var request CreatePlayerRequest

	err := json.NewDecoder(r.Body).Decode(&request)
	if err != nil {
		http.Error(w, err.Error(), http.StatusBadRequest)
		return
	}

	nicknameExist := isNicknameExist(request.Nickname)

	fmt.Printf("nicknameExist: %v\n", nicknameExist)
	if nicknameExist {
		http.Error(w, "Player already exists", http.StatusBadRequest)
		return
	}

	player := createPlayerEntity(request.Nickname)
	commandCenter := createCommandCenterEntity(player)

	player.CommandCenterId = commandCenter.Id
	player = updatePlayerEntity(player)

	w.Header().Set("Content-Type", "application/json")
	json.NewEncoder(w).Encode(player)
}

func isNicknameExist(nickname string) bool {
	var id string
	err := db.QueryRow("SELECT \"Id\" FROM \"Players\" WHERE \"Nickname\" = $1", nickname).Scan(&id)

	if err == sql.ErrNoRows {
		return false
	}

	if err != nil {
		panic(err)
	}

	return true
}

func createCommandCenterEntity(p Player) CommandCenter {
	var commandCenter CommandCenter
	commandCenter.Id = uuid.New().String()
	commandCenter.PlayerId = p.Id
	commandCenter.HighestDifficultyAvailable = Easy
	commandCenter.IsDeleted = false

	_, err := db.Exec("INSERT INTO \"CommandCenters\" (\"Id\", \"PlayerId\", \"HighestDifficultyAvailable\", \"IsDeleted\") VALUES ($1, $2, $3, $4)", commandCenter.Id, commandCenter.PlayerId, commandCenter.HighestDifficultyAvailable, commandCenter.IsDeleted)

	if err != nil {
		panic(err)
	}

	fmt.Println("CommandCenter created with commandCenter.Id: ", commandCenter.Id)

	return commandCenter
}

func createPlayerEntity(nickname string) Player {
	var player Player

	player.Id = uuid.New().String()
	player.Nickname = nickname
	player.Rank = Cadet
	player.Credits = 0
	player.Experience = 0
	player.IsDeleted = false

	_, err := db.Exec("INSERT INTO \"Players\" (\"Id\", \"Nickname\", \"Rank\", \"Credits\", \"Experience\", \"IsDeleted\") VALUES ($1, $2, $3, $4, $5, $6)", player.Id, player.Nickname, player.Rank, player.Credits, player.Experience, player.IsDeleted)
	if err != nil {
		panic(err)
	}

	fmt.Println("Player created with player.Id: ", player.Id)

	return player
}

func updatePlayerEntity(player Player) Player {
	_, err := db.Exec("UPDATE \"Players\" SET \"Nickname\" = $1, \"Rank\" = $2, \"Credits\" = $3, \"Experience\" = $4, \"CommandCenterId\" = $5 WHERE \"Id\" = $6", player.Nickname, player.Rank, player.Credits, player.Experience, player.CommandCenterId, player.Id)
	if err != nil {
		panic(err)
	}

	fmt.Println("Player updated with player.Id: ", player.Id)

	return player
}
