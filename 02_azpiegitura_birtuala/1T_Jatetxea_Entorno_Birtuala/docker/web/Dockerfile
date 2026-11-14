# Dockerfile for Apache + PHP 8.2 Web Frontend
FROM php:8.2-apache

# Install PDO MySQL and MySQLi extensions
RUN docker-php-ext-install mysqli pdo pdo_mysql && docker-php-ext-enable mysqli pdo_mysql

# Enable Apache rewrite module
RUN a2enmod rewrite

# Copy web files
COPY web_php/ /var/www/html/

# Permissions
RUN chown -R www-data:www-data /var/www/html

EXPOSE 80

CMD ["apache2-foreground"]
