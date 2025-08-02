import {writable} from "svelte/store";
import type {Card} from "$lib/models";

function readLocalStorageOrDefault<T>(key: string, defaultValue:T) {
    let value = defaultValue;

    if (typeof window === "undefined") {
        return value;
    }
    
    try
    {
        const json = localStorage.getItem(key);
        if (json !== null) {
            value = JSON.parse(json)
        }
    }
    catch (error)
    {
        console.error("Failed to read an object from the local storage", error);
    }
    
    return value;
}

function writeLocalStorage(key: string, value: string)
{
    if (typeof window === "undefined") {
        return;
    }
    
    localStorage.setItem(key, value);
}

function createPersistentWritable<T>(localStorageKey: string, defaultValue: T)
{
    const value = readLocalStorageOrDefault(localStorageKey, defaultValue);
    const wrapper = writable(value);
    wrapper.subscribe(v => writeLocalStorage(localStorageKey, JSON.stringify(v)));
    return wrapper;
}

export const cards = createPersistentWritable("cards", new Array<Card>());