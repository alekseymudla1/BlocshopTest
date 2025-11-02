# BlocshopTest
This implemented test task for Blocshop by Aleksey Mudla

## Task description
Ticket Reservation Service (no oversells)
Problem
Build a minimal ticket reservation API for events with fixed seat counts. Multiple users may try to reserve at once. The system must prevent overselling, support expiring holds, and emit domain events.
Core Concepts
• Event: { id, name, totalSeats, createdAt }
• Inventory: derived: availableSeats = totalSeats - confirmed - activeHolds
• Reservation Hold: temporary lock of N seats for a user; expires after TTL (e.g., 2 minutes).
• Reservation: confirmation that turns a valid hold into a purchase.
Requirements (MVP)
1. Endpoints (ASP.NET Core 9)
• POST /events – create event.
• GET /events/{id} – event detail incl. availableSeats, holdsActive, confirmedCount.
• POST /events/{id}/holds – start a hold: body { seats, customerId }.
• Must be idempotent via header Idempotency-Key.
• Return { holdId, expiresAt }.
• POST /events/{id}/holds/{holdId}/confirm – confirm hold → reservation.
• DELETE /events/{id}/holds/{holdId} – cancel hold.
• GET /events?search=&page=&pageSize= – list w/ pagination & filtering.
2. Concurrency & Consistency
• Prevent oversells under concurrent requests
• Holds expire automatically via a BackgroundService (sweeper) or database TTL pattern.
3. Idempotency
• Idempotency-Key header recorded per (route, customerId, key) → cached/DB to return same result on retry.



