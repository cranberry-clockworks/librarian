<script>
	import { CardSerializer } from "$lib/anki/card-serializer";
	import { MediaFolderProvider } from "$lib/anki/media-folder-provider";

    let dirs = MediaFolderProvider.getMediaFolders()
    // import fs from "fs"
    // let s = fs.createWriteStream("foo.txt");
    // CardSerializer.serialize(s, [{front: "FOO", back: "Bar", tags: ["1", "2"]}]);
</script>

<h1>Test</h1>

<ul>
    {#each dirs as dir }
        <li><code>{dir}</code></li>
    {/each}
</ul>