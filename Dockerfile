# ===== BUILDER =====
FROM node:22 AS builder
WORKDIR /usr/src/app

# copy package files first for layer caching
COPY package*.json ./

# Install all deps (dev + prod) for build & prisma generate
RUN npm ci

# copy source and prisma schema
COPY . .

# Generate Prisma client for the container's platform and build
RUN npx prisma generate
RUN npm run build

# ===== FINAL =====
FROM node:22 AS final
WORKDIR /usr/src/app

# Copy package files so we can install only production deps
COPY package*.json ./

# Install only production deps
RUN npm ci --omit=dev && npm cache clean --force

# Copy built app and Prisma artifacts from builder
COPY --from=builder /usr/src/app/dist ./dist
COPY --from=builder /usr/src/app/prisma ./prisma
# Prisma native binaries can live under node_modules/.prisma - copy them from builder
COPY --from=builder /usr/src/app/node_modules/.prisma ./node_modules/.prisma

# Copy other runtime files you need (e.g., prisma/schema, migrations)
COPY --from=builder /usr/src/app/prisma/migrations ./prisma/migrations

# Copy entrypoint (will run migrations in production) and make executable
COPY docker-entrypoint.sh ./
RUN chmod +x docker-entrypoint.sh

# Expose port (adjust if different)
EXPOSE 3000

# Use non-root user
USER node

ENTRYPOINT ["./docker-entrypoint.sh"]
CMD ["node", "dist/main.js"]
