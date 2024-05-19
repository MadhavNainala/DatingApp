import { HttpEvent, HttpHandler, HttpInterceptorFn, HttpRequest } from '@angular/common/http';
import { Observable, take } from 'rxjs';
import { observableToBeFn } from 'rxjs/internal/testing/TestScheduler';
import { AccountService } from '../_services/account.service';
import { inject } from '@angular/core';

export const jwtInterceptor: HttpInterceptorFn = (req : HttpRequest<any>, next: any) : Observable<any> => {
  const accountService = inject(AccountService);
  accountService.currentUser$.pipe(take(1)).subscribe({
    next: user => {
        if(user) {
          req = req.clone({
            setHeaders: {
              Authorization: `Bearer ${user.token}`
            }
            })
        }
    }
  })
  return next(req);
};
