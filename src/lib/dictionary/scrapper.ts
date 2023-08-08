import axios from 'axios';
import type { Substantiv, UnknownPartOfSpeech, Verb } from './definition';
import { load, type CheerioAPI } from 'cheerio';

export type Definition = UnknownPartOfSpeech | Substantiv | Verb;

export async function getDefinition(root: string): Promise<Definition> {
	const uri = `wiki/${encodeURIComponent(root)}`;

    console.debug("[scrapper] GET", uri)
	
    const result = await axios.get(uri);
    console.debug("[scrapper] Response", result.status)
	
    const html = result.data;
	const page = load(html);

    const partOfSpeech = detectPartOfSpeech(page);
    console.debug("[scrapper] Detected the part of speech", partOfSpeech)
    
    let definition;
	switch (partOfSpeech) {
		case 'Substantiv':
			definition = parseSubstantivePage(page);
            break;
		case 'Verb':
			definition = parseVerbPage(page);
            break;
		default:
			definition =  { root: root } as const;
            break;
	}

    console.debug("[scrapper] Extracted definition", definition);
    return definition;
}

type PartOfSpeech = 'Substantiv' | 'Verb' | unknown;

function detectPartOfSpeech(page: CheerioAPI): PartOfSpeech {
	if (page('#Substantiv').length > 0)
        return 'Substantiv';
	if (page('#Verb').length > 0) return 'Verb';

	return null;
}

function parseSubstantivePage(page: CheerioAPI): Substantiv {
	const table = page('table[class*="template-sv-subst"]').first();
	const root = table.find('tbody > tr:nth-child(3) > td:nth-child(2) > span > a').text().trim();
	const gender = table.find('tbody > tr:nth-child(2) > th.main').text().trim().toLowerCase();
	
	if (!(gender == 'utrum' || gender == 'neutrum')) throw new Error(`Uknown gender: ${gender}`);

	return {
		root: root,
		gender: gender
	} as const;
}

function parseVerbPage(page: CheerioAPI): Verb {
	const table = page('table[class*="template-sv-verb"]').first();
	return {
		infinitive: table
			.find('table > tbody > tr:nth-child(2) > td:nth-child(2) > span > a')
			.text()
			.trim(),
		presens: table.find('tbody > tr:nth-child(3) > td:nth-child(2) > span').text().trim(),
		preteritum: table.find('tbody > tr:nth-child(4) > td:nth-child(2) > span').text().trim(),
		supinum: table.find('tbody > tr:nth-child(5) > td:nth-child(2) > span').text().trim(),
		imperative: table.find('tbody > tr:nth-child(6) > td:nth-child(2) > span').text().trim()
	} as const;
}
