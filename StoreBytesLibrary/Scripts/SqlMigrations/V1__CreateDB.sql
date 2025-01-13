--CREATE DATABASE storebytesdb;

CREATE TABLE users (
    id SERIAL PRIMARY KEY,
    email VARCHAR(255) NOT NULL UNIQUE,
    created_at TIMESTAMP DEFAULT NOW() NOT NULL,
    is_active BOOLEAN DEFAULT true NOT NULL
);

CREATE TABLE user_tokens (
    id SERIAL PRIMARY KEY,
    user_id INT NOT NULL REFERENCES users(id),
    api_key VARCHAR(64) UNIQUE NOT NULL,
    description VARCHAR(255),
    created_at TIMESTAMP DEFAULT NOW() NOT NULL,
    expires_at TIMESTAMP,
    is_active BOOLEAN DEFAULT true NOT NULL
);

CREATE TABLE buckets (
    id SERIAL PRIMARY KEY,
  	user_id INT NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    name VARCHAR(255) NOT NULL,
    hashed_name VARCHAR(64) NOT NULL,
    created_at TIMESTAMP DEFAULT NOW() NOT NULL,
    is_active BOOLEAN DEFAULT true NOT NULL,
    UNIQUE (name, user_id)
);

CREATE TABLE files (
    id SERIAL PRIMARY KEY,
    bucket_id INT NOT NULL REFERENCES buckets(id) ON DELETE CASCADE,
    original_name VARCHAR(255) NOT NULL,
    hashed_name VARCHAR(64) NOT NULL,
    file_path TEXT NOT NULL,
    size BIGINT NOT NULL,
    content_type VARCHAR(255),
    created_at TIMESTAMP DEFAULT NOW() NOT NULL,
    UNIQUE (bucket_id, original_name)
);

