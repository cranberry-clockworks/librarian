/* generated using openapi-typescript-codegen -- do no edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { NounDefinition } from '../models/NounDefinition';
import type { UnknownDefinition } from '../models/UnknownDefinition';
import type { VerbDefinition } from '../models/VerbDefinition';

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
    ): CancelablePromise<Array<(NounDefinition | VerbDefinition | UnknownDefinition)>> {
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
