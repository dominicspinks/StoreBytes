# StoreBytes

StoreBytes API is a file storage service designed to provide bucket-based file management similar to AWS S3. The API allows users to create buckets, upload files, and access files via unique URLs. It also supports authentication via JWT tokens and API keys.

---

## Features

### Authentication
- **JWT Tokens**: Authenticate requests using short-lived tokens.
- **API Keys**: Generate API keys for authentication.

### Buckets
- Create buckets to organise files.
- Each bucket has a hashed name for obfuscation.

### File Management
- Upload files to specific buckets.
- Access files via unique, obfuscated URLs.

---

## Project Structure

```
StoreBytesAPI
├── Controllers
│   ├── AuthController.cs
│   └── FilesController.cs
├── Program.cs

StoreBytesLibrary
├── Data
│   ├── PGSqlData.cs
├── Databases
│   ├── PGSqlDataAccess.cs
├── Models
│   └── FileMetadata.cs
├── Services
│   └── FileStorageService.cs
```

---

## Prerequisites

### Software Requirements
- **.NET 8.0 SDK**
- **PostgreSQL**
- **Docker** (for deployment)

---

## Setup Instructions

### 1. Clone the Repository
```bash
git clone <repository-url>
cd StoreBytesAPI
```

### 2. Configure Environment Variables
Create a `.env` file in the root directory with the following variables:
```env
DATABASE_URL=Host=<host>;Port=<port>;Database=<db>;Username=<user>;Password=<password>
HASH_SECRET=<hashing-secret>
JWT_SECRET=<jwt-secret>
```

### 3. Set Up the Database
Run the SQL scripts located in `StoreBytesLibrary/Scripts/SqlMigrations/V1_CreateDB.sql` and `StoreBytesLibrary/Scripts/V1_CreateUser.sql` to create the required tables.

### 4. Run the Application

#### Development Mode
```bash
dotnet run
```

#### Docker Deployment
1. Build and run the Docker image:
	```bash
	docker build -t storebytes-api .
	docker run -d -p 8080:8080 --env-file .env --name storebytes-api storebytes-api
	```

2. Verify the container is running:
	```bash 
	docker ps
	```

---

## API Endpoints

### Authentication
- **POST /auth/token**: Generate a short-lived JWT token using an API key.
- **POST /auth/create-api-key**: Generate a new API key for the authorised user.

### Buckets
- **POST /buckets**: Create a new bucket.
- **GET /buckets**: Retrieve all buckets for the authorised user.

### Files
- **POST /files/upload**: Upload a file to a bucket.
- **GET /files/{bucketHash}/{fileHash}**: Retrieve a file via its unique URL.

---

## Testing

### Using Postman
1. Import the collection: Add API endpoints to Postman for testing.
2. Set up environment variables for Postman:
	- apiKey
	- `baseUrl`: Base URL of the API (e.g., `http://localhost:8080`).
	- `authToken`: JWT token obtained from `/auth/token`.

3. Test endpoints:
	- Generate an API key.
	- Use the API key to generate a JWT token.
	- Create a bucket and upload files.
	- Retrieve files using their URLs.

---

## Known Issues


---

## Future Enhancements
- Add support for file encryption at rest.
- Implement rate limiting for API requests.
- Add bucket and file lifecycle policies.

---

## License
This project is licensed under the MIT License.
