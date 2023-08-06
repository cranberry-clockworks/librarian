import type { WriteStream } from 'fs';
import type { Card } from './card';
import { v4 as uuid } from 'uuid';

export class CardSerializer {
	private static readonly Separator = '\t';
	private static readonly Prolog: Record<string, string> = {
		separator: 'Tab',
		html: 'true',
		'guid column': '1',
		'tags column': '2'
	};

	public static serialize(stream: WriteStream, cards: Iterable<Card>): void {
		this.writeProlog(stream);
		this.writeCards(stream, cards);
	}

	private static writeProlog(stream: WriteStream): void {
		for (const key in this.Prolog) {
			this.writePrologValue(stream, key, this.Prolog[key]);
		}
	}

	private static writePrologValue(stream: WriteStream, key: string, value: string): void {
		stream.write(`#${key}:${value}\n`);
	}

	private static writeCards(stream: WriteStream, cards: Iterable<Card>): void {
		for (const card of cards) {
			this.writeCard(stream, card);
		}
	}

	private static writeCard(stream: WriteStream, card: Card): void {
		const entry = [uuid(), card.tags.join(' '), card.front, card.back].join(this.Separator);

		stream.write(`${entry}\n`);
	}
}
