FROM node:22-alpine AS build
WORKDIR /app
COPY src/ArchQ.Web/package*.json ./
RUN npm ci
COPY src/ArchQ.Web/ .
RUN npx ng build --configuration production

FROM nginx:alpine
COPY --from=build /app/dist/arch-q-web/browser /usr/share/nginx/html
COPY docker/web/nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
