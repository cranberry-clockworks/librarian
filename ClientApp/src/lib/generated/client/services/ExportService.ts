/* generated using openapi-typescript-codegen -- do no edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { ExportRequest } from '../models/ExportRequest';

import type { CancelablePromise } from '../core/CancelablePromise';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';

export class ExportService {

    /**
     * Export cards
     * @param requestBody
     * @returns any OK
     * @throws ApiError
     */
    public static export(
        requestBody: ExportRequest,
    ): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/api/export',
            body: requestBody,
            mediaType: 'application/json',
        });
    }

}
