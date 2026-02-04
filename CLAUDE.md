# CLAUDE.md - AI Assistant Guide for Gender War Game

## Project Overview

**Gender War** is a satirical narrative game (visual novel style) about how online gender ideologies distort modern dating. It's a self-contained single-page web application with no external dependencies or build tools.

**Purpose**: Critical, darkly humorous exploration of incel/femcel subcultures through interactive dialogue choices and branching narratives.

**Tech Stack**: Pure HTML5 + CSS3 + Vanilla JavaScript (no frameworks, no npm, no build process)

## File Structure

```
/gender-war-game-beta/
├── index.html          # Entire game (HTML, CSS, JS - ~2,346 lines)
├── README.md           # Developer statement and project description
├── CLAUDE.md           # This file
└── sprites/            # Character and background assets
    ├── incel_neutral.png, incel_angry.png
    ├── femcel_neutral.png, femcel_smirk.png
    ├── performative_neutral.png, performative_hurt.png
    ├── bop_neutral.png, bop_present.png
    ├── player_neutral.png, player_concerned.png
    ├── waiter_neutral.png
    ├── bg_dialogue.png     # Main game background (2.4MB)
    └── title_bg.mp4        # Title screen video (6.8MB)
```

## Architecture

### Single-File Structure (index.html)

The entire game is contained in one HTML file with three main sections:

| Section | Lines | Description |
|---------|-------|-------------|
| HTML Structure | 1-728 | Screen containers, UI elements, dialogue boxes |
| CSS Styles | 10-726 | Inline `<style>` block with all styling |
| JavaScript | 962-2346 | Game logic, dialogue data, state management |

### Screen System

The game uses multiple full-screen containers (`.screen` class) toggled via JavaScript:

- `#screen-title` - Title screen with video background
- `#screen-creator` - Character customization (bunny avatar)
- `#screen-dateselect` - Part 1 date selection
- `#screen-dateselect2` - Part 2 date selection
- `#screen-game` - Main dialogue gameplay
- `#screen-ending` - Receipt/summary screen

### Game Flow

```
Title → Character Creator → Date Select (Part 1) → Dialogue → Ending Receipt
                                    ↓
                           Date Select (Part 2) → Dialogue → Ending Receipt
                                    ↓
                           Part 3 (Integration) → Final Ending
```

## Key Code Sections

### Dialogue Data (Lines ~964-1753)

Dialogue is stored as node-based trees in constant objects:

```javascript
const INCEL_ROUTE = {
  start: {
    speaker: "Kevin",
    text: "...",
    choices: [
      { label: "Response option", next: "node_id" }
    ],
    effect: { money: -5, patience: -1 },  // Optional stat changes
    receipt: { item: "Coffee", cost: 5 }, // Optional spending tracking
    sprite: "angry",                       // Optional expression change
    ending: true                           // Marks end of route
  }
};
```

**Route Constants**:
- `INCEL_ROUTE` (~50 nodes) - Kevin, the spreadsheet misogynist
- `FEMCEL_ROUTE` (~40 nodes) - Jessica, high-value woman archetype
- `PERFORMATIVE_ROUTE` (~40 nodes) - Brendan, overly-therapeutic male
- `BOP_ROUTE` (~50 nodes) - Madison, chaotic influencer
- `THEMCEL_ROUTE` (~15 nodes) - Part 3 integration/group acceptance

### State Management (Lines ~1756-1783)

```javascript
const State = {
  appearance: { skin, eyes, mouth, outfit },  // Character customization
  route: null,           // Current route (incel/femcel/performative/bop/themcel)
  part: 1,               // Game phase (1, 2, or 3)
  nodeId: null,          // Current dialogue node
  money: 100,            // Starting budget
  patience: 10,          // Tolerance meter
  receipt: [],           // Cost tracking array
  runLog: [],            // All choices made (for reputation analysis)
  reputation: {
    score: 0,
    tags: { /* boolean flags for player behavior */ }
  },
  part3Unlocked: false,
  integration: {
    legibility: 0,       // How predictable player is
    friction: 0,         // How disruptive to process
    reframingAcceptance: 0  // Willingness to conform
  }
};
```

### Core Functions (Lines ~1849-2284)

| Function | Purpose |
|----------|---------|
| `renderNode(nodeId)` | Display current dialogue and choices |
| `renderSprite(speaker, expression)` | Update character sprite images |
| `showScreen(id)` | Navigate between game screens |
| `startGame(route, part)` | Initialize new game session |
| `showEnding(node)` | Display end-of-date receipt |
| `updateStats()` | Refresh money/patience HUD |
| `computeReputationFromRunLog()` | Analyze choices for reputation tags |
| `getRumorLine()` | Generate contextual reputation text |
| `computeIntegration()` | Calculate Part 3 metrics |
| `checkAcceptance()` | Determine Part 3 ending |
| `openPhone()` / `closePhone()` | Toggle profile browsing modal |

### Event Initialization (Lines ~2286-2346)

Button event handlers are attached at the end of the script block using IIFEs for closure capture.

## Code Conventions

### JavaScript Style
- ES5/ES6 mix (uses `const`, `var`, arrow functions)
- No classes - functional programming with closures
- DOM access via `document.getElementById()` and `querySelectorAll()`
- Section comments: `// ===== SECTION NAME =====`

### CSS Patterns
- Dark color scheme: `#1a1216` (burgundy) with `#ff4d6d` (pink accent)
- `.hidden` class for visibility toggling
- `.active` class for screen switching
- Responsive breakpoint at `max-width: 700px`

### Data Patterns
- Dialogue nodes use object literals with ID-based navigation
- Dynamic text placeholders: `{RUMOR}`, `{INTEGRATION_RESULT}`
- Tag-based reputation system (keywords in choices analyzed at runtime)

## Development Workflow

### Running the Game
No build process required. Simply:
1. Open `index.html` in any modern browser
2. Or serve with any static file server

### Making Changes
1. Edit `index.html` directly
2. Refresh browser to see changes
3. No compilation or bundling needed

### Adding New Dialogue
1. Find the appropriate route constant (e.g., `INCEL_ROUTE`)
2. Add new node with unique ID
3. Connect to existing nodes via `choices[].next`
4. Test by playing through the route

### Adding New Characters/Routes
1. Create new route constant following existing pattern
2. Add sprite images to `/sprites/`
3. Update `SPRITES` mapping object
4. Add route selection button in HTML
5. Connect to screen flow logic

## Important Notes for AI Assistants

### Do
- Preserve the satirical tone when editing dialogue
- Maintain node ID uniqueness within routes
- Test stat effects (money/patience) don't create unwinnable states
- Keep sprite expressions consistent with dialogue mood
- Preserve the receipt system for spending tracking

### Don't
- Add external dependencies (keep it dependency-free)
- Split into multiple files (single-file architecture is intentional)
- Change the critique/satirical nature of content
- Remove the "uncomfortable by design" elements
- Add build tools or compilation steps

### Common Tasks
- **Add dialogue option**: Edit route constant, add choice object to node
- **Change character expression**: Update `sprite` property in node
- **Modify stat effects**: Edit `effect` object in node
- **Add receipt item**: Add `receipt` object to node
- **Fix responsive issues**: Edit media query in CSS section

## Known Issues / TODOs

From README.md:
- [ ] Fix sprites so they are consistent in size and framing
- [ ] Crop and clean backgrounds on all character sprites
- [ ] Link the game publicly for playtesting and feedback

## External Resources

- **Fonts**: Google Fonts (Special Elite, Crimson Pro, Space Mono) loaded via CDN
- **No other external dependencies**

## Quick Reference

### File Locations
- Main game logic: `index.html:962-2346`
- Dialogue data: `index.html:964-1753`
- Game state: `index.html:1756-1783`
- Sprite system: `index.html:1785-1846`
- UI functions: `index.html:1849-2284`
- CSS styles: `index.html:10-726`

### Character Sprite Naming
Format: `{character}_{expression}.png`
- Expressions: `neutral`, `angry`, `smirk`, `hurt`, `concerned`, `present`
