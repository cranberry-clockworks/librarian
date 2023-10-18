/* generated using openapi-typescript-codegen -- do no edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { CancelablePromise } from '../core/CancelablePromise';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';

export class TranslationsService {

    /**
     * Translates the phrase
     * @param phrase
     * @param requestBody
     * @returns string OK
     * @throws ApiError
     */
    public static translate(
        phrase: string,
        requestBody?: any,
    ): CancelablePromise<string> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/translation/{phrase}',
            path: {
                'phrase': phrase,
            },
            body: requestBody,
        });
    }

}
