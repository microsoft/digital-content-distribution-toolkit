FROM cblmariner.azurecr.io/base/core:1.0 as build-step
# manually install nodejs as pre-baked mariner-based image is not available
RUN tdnf -y install nodejs

# force cipher for nodejs due to compatibility issues with openssl. Tracking bug: https://microsoft.visualstudio.com/OS/_workitems/edit/36118736
ENV NODE_OPTIONS=--tls-cipher-list='ECDHE-RSA-AES128-GCM-SHA256:!RC4'

WORKDIR /app

COPY blendnet.cms.web.ui /app
RUN npm install
RUN npm run build-environment_name

# EXPOSE 4200
# CMD ["ng","serve","--host","0.0.0.0"]

FROM cblmariner.azurecr.io/base/core:1.0 as nginx
# manually install nginx as pre-baked mariner-based image is not available
RUN tdnf -y install nginx
RUN tdnf clean all

COPY --from=build-step /app/dist/CMS /usr/share/nginx/html
COPY blendnet.cms.web.ui/nginx.conf /etc/nginx/nginx.conf
# EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
