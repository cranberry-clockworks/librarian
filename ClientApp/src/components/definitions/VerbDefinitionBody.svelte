<script lang="ts">
    import type {Verb} from "$lib/generated/client";
    import DefinitionHeader from "./DefinitionHeader.svelte";

    export let definition: Verb
    
    export let inflecrtion = "";
    
    function strip(type: string | undefined) {
        return type?.replaceAll("<", "").replaceAll(">", "") ?? "";
    }
    
</script>

{#each definition.inflections ?? [] as inflection}
    <div style="margin-top: 20px">
        <DefinitionHeader isSub={true} phrase="{inflection.word}" wordClass="Verb ({strip(inflection.type)})" />
    </div>
{/each}

<style>
</style>

