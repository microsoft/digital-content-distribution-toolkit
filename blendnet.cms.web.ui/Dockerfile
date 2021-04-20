FROM node:12-buster-slim
WORKDIR /app

COPY blendnet.cms.web.ui /app
RUN npm install
RUN npm install -g @angular/cli
RUN ng build --prod

# EXPOSE 4200
CMD ["ng","serve","--host","0.0.0.0"]
