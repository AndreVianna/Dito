# Task 2.7: Search Recordings

**Goal:** Enable users to find specific recordings by searching their transcript content.
**Part of:** Delivery 2a

## Context
With many recordings, the list becomes unwieldy. Users remember *what* was said, not *when*. Full-text search is essential.

## Requirements

### Functional
- **Search Bar:** Prominently placed at the top of the recordings list.
- **Filtering:** Real-time filtering as the user types (debounced).
- **Scope:** Search both **Title** (if available/auto-generated) and **Transcript** body.
- **Match:** Case-insensitive partial match.
- **Feedback:** "No recordings found" message when list is empty due to search.
- **Clear:** 'X' button to clear search and restore full list.

### Technical
- **Query:** EF Core `DbSet<Recording>.Where(r => r.Transcript.Contains(searchTerm) || r.Title.Contains(searchTerm))`.
- **Performance:** Debounce keystrokes (300ms) to prevent UI lag/DB spam.
- **Snippet:** Display matching snippet in the list item (e.g., "...found keyword here..."). *Nice to have for MVP, required for v2.*

## Acceptance Criteria (Verification Steps)

### Scenario: Search for Keyword
- Create a recording with the transcript "The quick brown fox".
- Type "quick" into the search bar.
- Verify that the recording appears in the filtered list.
- Confirm that unrelated recordings are hidden from view.

### Scenario: Case Insensitive Search
- Create a recording with the transcript "Hello World".
- Type "hello" into the search bar.
- Verify that the recording "Hello World" appears in the results.

### Scenario: Clear Search
- Filter the list by typing "foobar" (or any search term).
- Click the "Clear" (X) button inside the search box.
- Verify that the search query is cleared.
- Confirm that all recordings are visible again.

### Scenario: No Results
- Type "randomwords123" (or a non-existent term) into the search bar.
- Ensure no recording contains that text.
- Verify that the list is empty.
- Confirm that a placeholder message "No recordings found" is displayed.
