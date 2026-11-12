# CodeAssessmentWeb

Angular frontend for the CodeAssessment product API. The product table supports name search, stock-range filtering, create/edit/delete, and stock adjustments.

## Run locally

Start the API using its `dev` launch profile (it listens on `http://localhost:5070`), then run:

```powershell
cd codeAssessmentWeb
npm install
npm start
```

Open `http://localhost:4200`. The Angular development server proxies `/api` requests to the API, so no backend CORS changes are required.

## Build and tests

```powershell
npm run build
npm test
```

The app uses the same-origin `/api/products` path. Production hosting should route `/api` to the API service.
