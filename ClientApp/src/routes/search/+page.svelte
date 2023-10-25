<script lang="ts">
    import {page} from "$app/stores";
    import {DefinitionsService} from "$lib/generated/client";
    import Definition from "../../components/Definition.svelte";
    
    async function search(word: string | null) {
        if (word === null)
            return [];
        
        return DefinitionsService.define(word);
    }
</script>


<p>
    {#await search($page.url.searchParams.get("q"))}
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