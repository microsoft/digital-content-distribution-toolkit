
const { createProxyMiddleware } = require('http-proxy-middleware');

module.exports = function(app) {
  app.use(
    '/ContentProviders/',
    createProxyMiddleware({
      target: 'http://localhost:54832/api/v1/',
       changeOrigin: true,
    })
  );

  app.use(
    '/Identity/',
    createProxyMiddleware({
      target: 'http://localhost:55939/api/v1/',
       changeOrigin: true,
    })
  );
};