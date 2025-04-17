# 🐙 Krakenar by Logitar

Krakenar is a tool suite aiming at handling non-business software requirements, allowing developers to focus on real, domain business requirements.

## Recommended IDE Setup

[VSCode](https://code.visualstudio.com/) + [Volar](https://marketplace.visualstudio.com/items?itemName=Vue.volar) (and disable Vetur).

## Type Support for `.vue` Imports in TS

TypeScript cannot handle type information for `.vue` imports by default, so we replace the `tsc` CLI with `vue-tsc` for type checking. In editors, we need [Volar](https://marketplace.visualstudio.com/items?itemName=Vue.volar) to make the TypeScript language service aware of `.vue` types.

## Customize configuration

See [Vite Configuration Reference](https://vite.dev/config/).

## Project Setup

```sh
npm install
```

### Compile and Hot-Reload for Development

```sh
npm run dev
```

### Type-Check, Compile and Minify for Production

```sh
npm run build
```

### Preview the Last Build from dist

```sh
npm run preview
```

### Run Unit Tests and Code Coverage with [Vitest](https://vitest.dev/)

```sh
npm run test
```

### Run Unit Tests with Vitest

```sh
npm run test:dev
```

### Build the Application Without Type Checking for Production

```sh
npm run build-only
```

### Type Check the Application Code Without Building

```sh
npm run type-check
```

### Lint with [ESLint](https://eslint.org/)

```sh
npm run lint
```

### Format with [Prettier](https://prettier.io/)

```sh
npm run format
```
