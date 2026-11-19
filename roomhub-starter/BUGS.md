# BUGS

The starter contains behaviors that contradict `ASSIGNMENT.md`. Write here every discrepancy you found.
Fill it in yourself, in your own words. There is no required number: what counts is that they are real.

| Which rule from ASSIGNMENT.md is violated (R1–R8) | Where the problem was in the starter (file, approximate line) | How you discovered it (test, reading, AI) | What you changed |
|---|---|---|---|
| R7 | `Services/BookingService.cs`, line 28 | Asked the AI and checked the response | The overlap check used `<=` (`existing.StartUtc <= endUtc && startUtc <= existing.EndUtc`), causing bookings that only touch at boundaries (e.g., 10:00–11:00 and 11:00–12:00) to be rejected with 409 Conflict. Changed `<=` to `<` so that touching intervals are allowed. |
| R7 / R8 | `Services/BookingService.cs`, lines 26–30 | Asked the AI and checked the response | The overlap check iterated over all bookings for the room without checking their status, meaning cancelled bookings still blocked the interval. Added a filter to only check active bookings (`existing.Status == BookingStatus.Active`), allowing cancelled intervals to be rebooked. |
| R8 | `Services/BookingService.cs`, lines 56–60 | Asked the AI and checked the response | `Cancel(id)` unconditionally assigned `booking.Status = BookingStatus.Cancelled` without checking whether it was already cancelled, returning 204 NoContent on repeat calls. Added a check: if the booking status is already `Cancelled`, return a 409 Conflict error. |
| R5 | `Services/BookingService.cs`, `Create` method (lines 10–25) |Asked the AI and checked the response | There was no validation preventing bookings from starting in the past, and `TimeProvider` was not used anywhere in the starter. Added validation ensuring `startUtc` is not in the past relative to `TimeProvider.GetUtcNow()`, returning 400 Bad Request if violated. |
| R6 | `Services/BookingService.cs`, `Create` method (lines 18–20) |Asked the AI and checked the response | The starter only checked `endUtc <= startUtc` and lacked any duration constraint. Added validation ensuring that the duration `(endUtc - startUtc) <= TimeSpan.FromHours(4)`, returning 400 Bad Request if the duration exceeds 4 hours. |
