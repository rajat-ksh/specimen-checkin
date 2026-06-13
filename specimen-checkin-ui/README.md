# specimen-checkin-ui

Frontend application for the Specimen Check-in system. Built with Vue 3, TypeScript and Vite as a fast development and build toolchain.

Tech stack
- Vue 3 (Composition API / `<script setup>`)
- TypeScript
- Vite (dev server, build, preview)
- Optional tools: ESLint, Prettier, Vitest (if configured in the project)

Prerequisites
- Node.js (recommended LTS >= 18)
- npm (bundled with Node) or `yarn` / `pnpm`

Verify locally:
- `node -v`
- `npm -v` (or `yarn -v`, `pnpm -v`)

Project setup
1. Clone the repository and open the frontend folder:
   - `git clone https://github.com/rajat-ksh/specimen-checkin.git`
   - `cd specimen-checkin/specimen-checkin-ui`
2. Install dependencies:
   - `npm install`
   - or `yarn` / `pnpm install`

Running the app (development)
- Start the Vite dev server with Hot Module Replacement (HMR):
  - `npm run dev`
  - or `yarn dev` / `pnpm dev`
- The dev server default URL is typically `http://localhost:5173` — check the terminal output.

Build and preview (production)
- Build for production:
  - `npm run build`
- Preview the production build locally:
  - `npm run preview`

Common scripts
- `dev` — start development server
- `build` — production build into `dist/`
- `preview` — serve production build locally
- `lint` — run linting (if configured)
- `test` — run unit tests (if configured)

Environment variables
- Use Vite `.env` files in the project root. Prefix client-exposed vars with `VITE_`.
  - Example: `VITE_API_BASE_URL=https://api.example.com`
- Access in code using `import.meta.env.VITE_API_BASE_URL`.

Project structure (typical)
- `index.html` — Vite entry
- `src/`
  - `main.ts` — app bootstrap
  - `App.vue`
  - `components/` — reusable components
  - `views/` — page-level components
  - `assets/` — images, styles
- `public/` — static files copied to build output
- `vite.config.ts` — Vite configuration

How to use the application
- Configure backend API base URL in `.env` as `VITE_API_BASE_URL` (point to the Specimen Check-in API).
- Start the dev server and navigate to the app in the browser. Use the UI to check-in specimens according to the app flows implemented in `src/views`.

Deployment
- Build with `npm run build` and deploy the contents of `dist/` to static hosting (Netlify, Vercel, GitHub Pages, S3 + CloudFront).
- If deploying to a subpath, set `base` in `vite.config.ts` or pass `--base` to the build command.

Troubleshooting
- Dev server not starting: ensure Node version meets the requirement and reinstall dependencies (`rm -rf node_modules && npm install`).
- HMR not working: confirm there is no port conflict or proxy blocking websockets.
- Type errors: run TypeScript type checks or open `tsconfig.json` to adjust settings.

Contributing
- Create feature branches from `main` and open a pull request. Follow any existing `CONTRIBUTING.md` or repository guidelines.

References
- Vue: https://vuejs.org
- Vite: https://vitejs.dev
- TypeScript: https://www.typescriptlang.org

Repository
- https://github.com/rajat-ksh/specimen-checkin
