FROM node:17-alpine as build
WORKDIR /app

EXPOSE 3000

ARG DEBIAN_FRONTEND=noninteractive

# Dependencies
#RUN apt-get -qq update && apt-get -qq install -y wget
#RUN npm install -g yarn

# Package installation
COPY package.json yarn.lock .

RUN yarn

COPY . .

ENTRYPOINT ["./start.sh"]
