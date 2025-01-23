ALTER TABLE users ADD COLUMN password_hash VARCHAR(60);

ALTER TABLE files RENAME COLUMN original_name TO file_name;
ALTER TABLE files RENAME COLUMN hashed_name TO file_hash;
ALTER TABLE buckets RENAME COLUMN name TO bucket_name;
ALTER TABLE buckets RENAME COLUMN hashed_name TO bucket_hash;
ALTER TABLE buckets ADD CONSTRAINT unique_bucket_hash UNIQUE (bucket_hash);

ALTER TABLE user_tokens RENAME TO keys;
ALTER INDEX user_tokens_api_key_key RENAME TO keys_api_key_key;
ALTER INDEX user_tokens_pkey RENAME TO keys_pkey;
ALTER TABLE keys RENAME CONSTRAINT user_tokens_user_id_fkey TO keys_user_id_fkey;
ALTER SEQUENCE user_tokens_id_seq RENAME TO keys_id_seq;

ALTER TABLE keys RENAME COLUMN api_key TO key_hash;