<script lang="ts">
    import {page} from "$app/stores";
    import {DefinitionsService} from "$lib/generated/client";
    
    async function search(word: string | null) {
        if (word === null)
            return;
        
        let array = await DefinitionsService.define(word);
    }
</script>


<p>
    {#await search($page.url.searchParams.get("q"))}
        Loading...
    {:then word}
        The word is {word}
    {:catch error}
        There is an error: ${error}
    {/await}
</p>