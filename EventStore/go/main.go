package main

import (
	"encoding/json"
	"fmt"
	"log"
	"os"
	"os/signal"
	"syscall"
	"time"

	"github.com/confluentinc/confluent-kafka-go/kafka"
)

type MissionCompleted struct {
	Id             string   `json:"Id"`
	Difficulty     int      `json:"Difficulty"`
	Status         string   `json:"Status"`
	Reinforcements int      `json:"Reinforcements"`
	Squad          []string `json:"Squad"`
	Timestamp      string   `json:"Timestamp"`
}

type Reward struct {
	Experience int `json:"Experience"`
	Credits    int `json:"Credits"`
}

type RewardGranted struct {
	PlayerId   string `json:"PlayerId"`
	Experience int    `json:"Experience"`
	Credits    int    `json:"Credits"`
	Difficulty int    `json:"Difficulty"`
	Timestamp  string `json:"Timestamp"`
}

func main() {
	consumeTopic := "MissionCompleted"
	produceTopic := "RewardGranted"
	consumer, err := kafka.NewConsumer(&kafka.ConfigMap{
		"bootstrap.servers": "broker:19092",
		"group.id":          "consumer-group-1",
		"auto.offset.reset": "earliest",
	})

	if err != nil {
		log.Fatalf("Error creating consumer: %s", err)
	}

	defer consumer.Close()

	producer, err := kafka.NewProducer(&kafka.ConfigMap{
		"bootstrap.servers": "broker:19092",
	})
	if err != nil {
		log.Fatalf("Error creating producer: %s", err)
	}
	defer producer.Close()

	err = consumer.Subscribe(consumeTopic, nil)
	if err != nil {
		log.Fatalf("Error subscribing to topic: %s", err)
	}

	sigchan := make(chan os.Signal, 1)
	signal.Notify(sigchan, syscall.SIGINT, syscall.SIGTERM)

	run := true

	for run {
		select {
		case sig := <-sigchan:
			fmt.Printf("Caught signal %v: terminating\n", sig)
			run = false
		default:
			ev := consumer.Poll(100)
			switch e := ev.(type) {
			case *kafka.Message:
				var message MissionCompleted
				err := json.Unmarshal(e.Value, &message)
				if err != nil {
					fmt.Printf("Error unmarshaling JSON: %v\n", err)
				} else {
					fmt.Printf("Consumed event from topic %s: key = %s value = %+v\n", *e.TopicPartition.Topic, string(e.Key), message)
					reward := Reward{
						Experience: 100,
						Credits:    100,
					}
					for i := 0; i < len(message.Squad); i++ {
						rewardMessage := RewardGranted{
							PlayerId:   message.Squad[i],
							Experience: reward.Experience,
							Credits:    reward.Credits,
							Difficulty: message.Difficulty,
							Timestamp:  time.Now().Format(time.RFC3339),
						}
						rewardValue, err := json.Marshal(rewardMessage)
						if err != nil {
							fmt.Printf("Error marshaling reward JSON: %v\n", err)
						} else {
							producer.Produce(&kafka.Message{
								TopicPartition: kafka.TopicPartition{Topic: &produceTopic, Partition: kafka.PartitionAny},
								Value:          rewardValue,
							}, nil)
							fmt.Printf("Produced reward event: %+v\n", rewardMessage)
						}
					}
				}
			case kafka.Error:
				fmt.Printf("Error: %v\n", e)
				run = false
			default:
				// Ignore other event types
			}
		}
	}
}
