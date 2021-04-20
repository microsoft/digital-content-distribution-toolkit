FROM node:10-buster

ENV NODE_ENV=production

WORKDIR /usr/src/app

# COPY ["package.json", "package-lock.json*", "npm-shrinkwrap.json*", "./"]
# RUN npm install --production --silent && mv node_modules ../

COPY blendnet.cms.web.ui .

RUN npm install -g @angular/cli
RUN npm install
RUN ng build --prod

# EXPOSE 4200

CMD ["ng serve -H 0.0.0.0 "]
