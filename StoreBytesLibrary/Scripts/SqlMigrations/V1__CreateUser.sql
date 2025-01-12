
-- Create the storebytes user with a secure password
CREATE USER storebytes WITH PASSWORD 'enter-password-here';

-- Grant CONNECT permission to the target database
GRANT CONNECT ON DATABASE storebytesdb TO storebytes;

-- Switch to the target database
\c storebytesdb;

-- Grant USAGE on the public schema
GRANT USAGE ON SCHEMA public TO storebytes;

-- Grant permissions on existing tables in the public schema
GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO storebytes;

-- Set default privileges for future tables in the public schema
ALTER DEFAULT PRIVILEGES IN SCHEMA public
GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO storebytes;

-- Grant EXECUTE permission for all existing functions and procedures
GRANT EXECUTE ON ALL FUNCTIONS IN SCHEMA public TO storebytes;
GRANT EXECUTE ON ALL PROCEDURES IN SCHEMA public TO storebytes;

-- Set default privileges for future functions and procedures in the public schema
ALTER DEFAULT PRIVILEGES IN SCHEMA public
GRANT EXECUTE ON FUNCTIONS TO storebytes;

-- Restrict access to other databases
REVOKE CONNECT ON DATABASE postgres FROM storebytes;