# BUG-087: Unknown routes show blank page instead of redirecting

## Summary
Navigating to any undefined route (e.g., `/nonexistent-page`) shows a completely blank main content area with no error message, 404 indication, or redirect. The sidebar renders but the main content is empty.

## Actual Behavior
- URL `/nonexistent-page` renders the sidebar but main content is entirely blank
- No error message, no redirect, no user guidance

## Expected Behavior
- Unknown routes should redirect to `/adrs` (the default authenticated route)
- Or show a proper 404 page with a link back to the dashboard

## Root Cause
The Angular route configuration in `app.routes.ts` has no wildcard catch-all route (`path: '**'`). Only the empty path `''` redirects to `/adrs`. Any other unmatched path renders the app layout with no routed component.

## Fix
Add a wildcard route `{ path: '**', redirectTo: '/adrs' }` at the end of the routes array.

## Severity
Medium — users who mistype a URL or follow a stale link see a blank page with no way to recover except manually editing the URL

## Steps to Reproduce
1. Log in to the application
2. Navigate to any non-existent URL (e.g., `/nonexistent-page`, `/foo/bar`)
3. Observe: sidebar renders but main content is completely blank
