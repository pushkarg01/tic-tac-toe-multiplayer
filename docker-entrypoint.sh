#!/bin/sh
set -e

# Entrypoint with retry + exponential backoff for Prisma migrations.
# Behavior:
# - If DATABASE_URL is unset -> skip migrations
# - Otherwise try `npx prisma migrate deploy` up to $MIGRATE_MAX_RETRIES times
#   with exponential backoff starting at $MIGRATE_INITIAL_BACKOFF seconds.
#
# Env (optional):
# MIGRATE_MAX_RETRIES (default 30)
# MIGRATE_INITIAL_BACKOFF (default 1)

MIGRATE_MAX_RETRIES=${MIGRATE_MAX_RETRIES:-30}
MIGRATE_INITIAL_BACKOFF=${MIGRATE_INITIAL_BACKOFF:-1}
MIGRATE_BACKOFF_CAP=${MIGRATE_BACKOFF_CAP:-30}

log() {
  printf '%s %s\n' "$(date --iso-8601=seconds 2>/dev/null || date '+%Y-%m-%dT%H:%M:%S')" "$*"
}

if [ -z "$DATABASE_URL" ]; then
  log "DATABASE_URL not set — skipping Prisma migrations."
else
  log "DATABASE_URL detected — attempting to apply Prisma migrations (prisma migrate deploy)."
  attempt=0
  backoff=$MIGRATE_INITIAL_BACKOFF

  while [ "$attempt" -lt "$MIGRATE_MAX_RETRIES" ]; do
    attempt=$((attempt + 1))
    log "Migration attempt $attempt/$MIGRATE_MAX_RETRIES..."

    # Run migration (non-interactive). If successful, break loop.
    if npx prisma migrate deploy; then
      log "Migrations applied successfully."
      break
    else
      log "Migrate attempt $attempt failed. Will retry after ${backoff}s."
      sleep "$backoff"
      # exponential backoff with cap
      backoff=$((backoff * 2))
      if [ "$backoff" -gt "$MIGRATE_BACKOFF_CAP" ]; then
        backoff=$MIGRATE_BACKOFF_CAP
      fi
    fi
  done

  if [ "$attempt" -ge "$MIGRATE_MAX_RETRIES" ]; then
    log "ERROR: reached max migration attempts ($MIGRATE_MAX_RETRIES). Exiting with failure."
    exit 1
  fi
fi

# Exec the CMD (proper PID 1, signal forwarding)
exec "$@"
