FROM node:20-alpine

WORKDIR /app

COPY docker/chat/package.json ./
RUN npm install --production

COPY docker/chat/server.js ./

EXPOSE 3001

HEALTHCHECK --interval=15s --timeout=3s --retries=3 \
  CMD wget --no-verbose --tries=1 --spider http://127.0.0.1:3001/health || exit 1

CMD ["node", "server.js"]
