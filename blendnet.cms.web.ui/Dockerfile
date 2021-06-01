FROM node:12-buster-slim as build-step
WORKDIR /app

COPY blendnet.cms.web.ui /app
RUN npm install
RUN npm install -g @angular/cli
RUN ng build --configuration environment_name

# EXPOSE 4200
# CMD ["ng","serve","--host","0.0.0.0"]

FROM nginx:latest
COPY --from=build-step /app/dist/CMS /usr/share/nginx/html
COPY blendnet.cms.web.ui/nginx.conf /etc/nginx/nginx.conf
# EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
