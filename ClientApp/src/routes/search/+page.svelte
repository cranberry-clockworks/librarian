<script lang="ts">
    import {page} from "$app/stores";
    import {DefinitionsService} from "$lib/generated/client";
    import Definition from "../../components/definitions/Definition.svelte";
    
    async function search(word: string | null, pos: string | null) {
        if (word === null)
            return [];
        
        let partOfSpeech = pos === "any" ? undefined : pos;
        return DefinitionsService.define(word, partOfSpeech ?? undefined, 3);
    }
</script>


<p>
    {#await search($page.url.searchParams.get("q"), $page.url.searchParams.get("pos"))}
        Loading...
    {:then definitions}
        <div>
            {#each definitions as definition}
                <Definition definition={definition} />
            {/each}
        </div>
    {:catch error}
        There is an error: ${error}
    {/await}
</p>