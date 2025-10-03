# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is the LifeTimer marketing website - an Angular 19 application that serves as a documentation and download portal for the LifeTimer desktop application. The site includes:

- **Home page** - Main landing page
- **Store page** - Download/purchase interface
- **Documentation** - Dynamic markdown content with TOC
- **Help** - Support resources

## Development Commands

```bash
# Start development server (hosts on all interfaces)
npm start

# Build for production
npm run build

# Run unit tests with Karma
npm test

# Build in watch mode for development
npm run watch
```

## Architecture

### Routing Structure

- Uses standalone Angular components with routing in `src/app/app.routes.ts`
- Main routes: `/home`, `/store`, `/docs`, `/help`
- Documentation has nested routing for dynamic content loading

### Layout System

Two main layouts:

- `MainLayoutComponent` - Standard pages (home, store, help)
- `DocsLayoutComponent` - Documentation pages with sidebar

### Documentation System

- Markdown content served from `/public/docs/` directory
- `MarkdownService` handles loading and error handling
- Uses `ngx-markdown` with GitHub Flavored Markdown and heading IDs
- `DocumentContentComponent` renders markdown dynamically
- `TocComponent` generates table of contents from markdown headings

### Key Services

- `MarkdownService` - Loads documentation from `/docs/document.md` endpoint

### Styling

- Uses Tailwind CSS with custom configuration
- SCSS for component-specific styles
- Custom font family: Raleway as primary sans-serif

### Dependencies

- Angular 19 with Angular Material and CDK
- ngx-markdown for documentation rendering
- marked-gfm-heading-id for heading ID generation
- Tailwind CSS for styling

## File Structure

```
src/
├── app/
│   ├── layout/          # Layout components
│   ├── pages/           # Page components
│   │   ├── docs/        # Documentation components
│   │   │   └── components/  # TOC and content renderers
│   │   ├── home/
│   │   ├── store/
│   │   └── help/
│   ├── services/        # Angular services
│   └── shared/          # Shared components (navbar, footer)
└── public/docs/         # Static markdown documentation files
```

## Testing

Uses Karma with Jasmine for unit testing. Test files follow `.spec.ts` naming convention.
