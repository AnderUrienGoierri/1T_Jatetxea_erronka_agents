# Dockerfile for MySQL 8.0 Database (jatetxea_db)
FROM mysql:8.0

# Set environment defaults
ENV MYSQL_DATABASE=jatetxea_db
ENV MYSQL_ROOT_PASSWORD=root

# Copy initial database schema
COPY espezifikazioak/datu_basea_schema.sql /docker-entrypoint-initdb.d/01_schema.sql

# Optimization and encoding parameters
CMD ["--default-authentication-plugin=mysql_native_password", "--character-set-server=utf8mb4", "--collation-server=utf8mb4_0900_ai_ci"]

EXPOSE 3306
