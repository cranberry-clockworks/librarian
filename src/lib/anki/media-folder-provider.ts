import fs from 'fs';
import path from 'path';

export class MediaFolderProvider {
	public static getMediaFolders(): string[] {
		const root = this.getRootFolder();
		return fs
			.readdirSync(root, { recursive: true })
			.filter((item) => typeof item === 'string')
			.map((item) => path.join(root, <string>item))
			.filter(
				(item) => fs.statSync(item).isDirectory() && path.basename(item) === 'collection.media'
			);
	}

	private static getRootFolder(): string {
		if (process.platform === 'darwin') {
			return path.join(process.env.HOME || '', 'Library/Application Support/Anki2');
		}

		if (process.platform === 'win32') {
			return path.join(process.env.APPDATA || '', 'Anki2');
		}

		throw new Error('The current OS is not supported.');
	}
}
