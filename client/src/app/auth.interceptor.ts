import { HttpInterceptorFn } from '@angular/common/http';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const token = localStorage.getItem('authToken');

  if (token) {
    req = req.clone({
      setHeaders: { Authorization: `Bearer ${token}` }
    });
  }

  console.log('Outgoing request:', {
    url: req.url,
    method: req.method,
    headers: req.headers.keys().map(k => ({ [k]: req.headers.get(k) }))
  });

  return next(req);
};
