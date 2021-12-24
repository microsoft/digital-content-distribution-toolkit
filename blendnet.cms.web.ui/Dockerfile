FROM cblmariner.azurecr.io/base/core:1.0 as build-step
# manually install nodejs as pre-baked mariner-based image is not available
RUN tdnf -y install ca-certificates
RUN tdnf -y install nodejs

WORKDIR /app

COPY blendnet.cms.web.ui /app
RUN npm install
RUN npm run build-environment_name

FROM cblmariner.azurecr.io/base/core:1.0 AS installer

RUN tdnf install -y dnf

RUN mkdir /staging \
    && dnf install -y --releasever=1.0 --installroot /staging nginx

# Clean up staging
RUN rm -rf /staging/etc/dnf \
    && rm -rf /staging/run/* \
    && rm -rf /staging/var/cache/dnf \
    && find /staging/var/log -type f -size +0 -delete

FROM cblmariner.azurecr.io/distroless/minimal:1.0 AS nginx

COPY --from=installer /staging/ /

# use unprivileged user
USER nobody



FROM nginx as publish

# Now copy the actual portal drop
COPY --from=build-step /app/dist/CMS /usr/share/nginx/html
COPY blendnet.cms.web.ui/nginx.conf /etc/nginx/nginx.conf

EXPOSE 5000
CMD ["nginx", "-p", "/tmp", "-e", "/tmp/error_log.log", "-g", "daemon off;"]
