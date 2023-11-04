/* generated using openapi-typescript-codegen -- do no edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { Definition } from '../models/Definition';

import type { CancelablePromise } from '../core/CancelablePromise';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';

export class DefinitionsService {

    /**
     * Defines the word
     * @param word
     * @param partOfSpeech
     * @param count
     * @param requestBody
     * @returns Definition OK
     * @throws ApiError
     */
    public static define(
        word: string,
        partOfSpeech?: string,
        count?: number,
        requestBody?: any,
    ): CancelablePromise<Array<Definition>> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/definition/{word}',
            path: {
                'word': word,
            },
            query: {
                'partOfSpeech': partOfSpeech,
                'count': count,
            },
            body: requestBody,
            errors: {
                400: `Bad Request`,
            },
        });
    }

}
