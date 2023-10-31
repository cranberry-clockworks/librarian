/* generated using openapi-typescript-codegen -- do no edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { Adjective } from '../models/Adjective';
import type { Noun } from '../models/Noun';
import type { Phrase } from '../models/Phrase';
import type { Verb } from '../models/Verb';

import type { CancelablePromise } from '../core/CancelablePromise';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';

export class DefinitionsService {

    /**
     * Defines the word
     * @param word
     * @param requestBody
     * @returns any OK
     * @throws ApiError
     */
    public static define(
        word: string,
        requestBody?: any,
    ): CancelablePromise<Array<(Phrase | Noun | Verb | Adjective)>> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/definition/{word}',
            path: {
                'word': word,
            },
            body: requestBody,
        });
    }

}
