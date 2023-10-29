/* generated using openapi-typescript-codegen -- do no edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { ExportRequest } from '../models/ExportRequest';

import type { CancelablePromise } from '../core/CancelablePromise';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';

export class AnkiService {

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
            url: '/api/anki/export',
            body: requestBody,
            mediaType: 'application/json',
        });
    }

    /**
     * Get available decks
     * @param requestBody
     * @returns string OK
     * @throws ApiError
     */
    public static getDecks(
        requestBody?: any,
    ): CancelablePromise<Array<string>> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/anki/decks',
            body: requestBody,
        });
    }

}
