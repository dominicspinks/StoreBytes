# StoreBytes

StoreBytes is an object storage service that provides bucket-based file management. Ideal for personal projects, it offers a user-friendly web interface and API integration for easy file storage and retrieval. The platform is designed to be self-hosted, making it a cost-effective alternative to cloud providers by avoiding unexpected fees while maintaining control over your data.

---

## Features

### Web Interface
The web interface allows users to manage their storage buckets and files easily. It provides an intuitive interface to:
- Create and view buckets.
- Retrieve and download files.
- Manage API keys

Sign up and try it out for yourself: [https://storebytes.domsapps.com](https://storebytes.domsapps.com)

### API Integration
The API supports:
- **Authentication**: Generate and use JWT tokens to authenticate requests.
- **File Upload**: Upload files directly to specific buckets.
- **File Access**: Retrieve files using unique, obfuscated URLs.

Access the API: [https://storebytesapi.domsapps.com](https://storebytesapi.domsapps.com)

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

#### API
Create a `.env` file in the root directory with the following variables:
```env
DATABASE_URL=Host=<host>;Port=<port>;Database=<db>;Username=<user>;Password=<password>
HASH_SECRET=<hashing-secret>
JWT_SECRET=<jwt-secret>
```

#### Web
Create a `.env` file in the root directory with the following variables:
```env
STOREBYTESAPI_URL=<url for your storebytes api>
JWT_SECRET=<jwt-secret>
```



### 3. Set Up the Database
Run the SQL scripts located in `StoreBytesLibrary/Scripts/SqlMigrations/` to create the required tables.
- `V1_CreateDB.sql`
- `V1_CreateUser.sql`
- `V1.1.sql`

### 4. Run the Application

#### Docker Deployment
1. Build and run the Docker images:
    ```bash
    docker build -f ./StoreBytes.API/Dockerfile -t storebytes-api ./StoreBytes.API
    docker run -d -p 8000:8080 -p 8001:8081 --env-file .env --name storebytes-api storebytes-api
    
    docker build -f ./StoreBytes.Web/Dockerfile -t storebytes-web ./StoreBytes.Web
    docker run -d -p 8010:8080 -p 8011:8081 --env-file .env --name storebytes-web storebytes-web
    ```

2. Verify the containers are running:
    ```bash
    docker ps
    ```

Alternatively, you can download the current images from Docker Hub:
- `dominicspinks/storebytesapi:1.1.0`
- `dominicspinks/storebytesweb:1.1.0`

---

## API Instructions

### Authentication: Generate a JWT Token
**POST /auth/token**

Use this endpoint to generate a short-lived JWT token using an API key.

#### Request:
```json
{
  "apiKey": "your-api-key"
}
```

#### Response:
```json
{
  "token": "generated-jwt-token"
}
```

### File Upload: Upload a File
**POST /files/upload**

Use this endpoint to upload a file to a specific bucket.

#### Request:
- Headers:
    - `Authorization: Bearer <jwt-token>`
- Body (Form-Data):
    - `bucketName`: Name of the bucket.
    - `file`: The file to upload.

#### Response:
```json
{
  "url": "shortened-url-for-file"
}
```

### File Retrieval: Access a File
**GET /files/{bucketHash}/{fileHash}**

Use this endpoint to retrieve a file using its unique URL.

#### Request:
```http
GET /files/{bucketHash}/{fileHash}
```

#### Response:
The file will be returned as binary data.

---

## License
This project is licensed under the MIT License.

