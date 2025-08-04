# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Pronetheia Healthcare AI is a branding and logo project focused on creating visual assets for a healthcare AI system. The project contains React components for animated and static logo implementations, along with supporting HTML and SVG files.

## Architecture and Structure

The codebase consists of:
- **React Components**: Two TypeScript React components (`PronetheiaLogoAnimated.tsx`, `PronetheiaLogoStatic.tsx`) that render SVG-based logos
- **HTML Demonstration**: A standalone HTML file (`collaborative_eye_logo.html`) showcasing both logo versions with usage guidelines
- **SVG Assets**: Raw SVG files for both animated and static logo versions
- **Utility**: An HTML-based SVG to PNG converter tool

## Key Design Patterns

### Logo Implementation
The logos use an eye metaphor with collaborative AI agents arranged in a ring around the iris:
- **Central Core**: Represents the main AI system (pupil with processing indicators)
- **Agent Ring**: Six specialized bots (Analyze, Predict, Decide, Alert, Learn, Watch) positioned around the iris
- **Data Flow**: Animated or static lines showing collaboration between agents

### Component Structure
Both React components follow similar patterns:
- TypeScript interfaces for props (width, height, className)
- Default prop values
- Inline SVG rendering with viewBox for scalability
- CSS animations defined within `<style>` tags (animated version)

### Color Palette
- Primary Blue: `#1B4B7C`
- Accent Orange: `#F4A261`
- Secondary Green: `#6B8E7A`
- Alert Red: `#E76F51`
- Neutral Gray: `#8D9AAE`

## Development Notes

Since this is a branding/visual asset project without a traditional build system:
- No package.json or build configuration exists
- Components are designed to be copied into existing React projects
- HTML files can be opened directly in browsers for demonstration
- SVG files can be used as standalone assets

## Working with Logo Components

When integrating the React components:
1. Import the component into your React project
2. Components accept optional props: `width`, `height`, `className`
3. The static version also accepts `showLabels` prop to toggle agent labels
4. Both components maintain aspect ratio through viewBox settings

## Asset Usage

- **Animated Logo**: Best for digital interfaces, loading screens, and interactive applications
- **Static Logo**: Ideal for print materials, documents, and situations requiring clarity at small sizes
- **SVG Files**: Can be used directly in web projects or converted to other formats as needed