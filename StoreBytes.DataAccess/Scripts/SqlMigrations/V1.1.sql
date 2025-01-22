ALTER TABLE users ADD COLUMN password_hash VARCHAR(60);

ALTER TABLE files RENAME COLUMN original_name TO file_name;
ALTER TABLE files RENAME COLUMN hashed_name TO file_hash;
ALTER TABLE buckets RENAME COLUMN name TO bucket_name;
ALTER TABLE buckets RENAME COLUMN hashed_name TO bucket_hash;
ALTER TABLE buckets ADD CONSTRAINT unique_bucket_hash UNIQUE (bucket_hash);