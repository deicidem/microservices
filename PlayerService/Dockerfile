# Use an official Golang image to build the app
FROM golang:1.20-alpine as builder

# Install git
RUN apk --no-cache update && \
    apk --no-cache add git gcc libc-dev

# Set the current working directory inside the container
WORKDIR /app

# Copy the go mod and sum files
COPY go.mod go.sum ./

# Download and cache dependencies
RUN go mod download

# Copy the source code
COPY . .

# Build the Go app
RUN go build -o consumer main.go

# Use a smaller base image for the runtime
FROM alpine:3.17

# Set the current working directory inside the container
WORKDIR /app

# Copy the binary from the builder stage
COPY --from=builder /app/consumer .

# Command to run the executable
CMD ["./consumer"]