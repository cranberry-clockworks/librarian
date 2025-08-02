<script lang="ts">
    import type {Definition} from "$lib/generated/client";

    import DefinitionEntry from "./DefinitionEntry.svelte";
    
    export let definition: Definition
    
    function getPhrase(d: Definition) {
        return (d.prefix ?? "").length > 0 ? `${d.prefix} ${d.lemma}` : d.lemma;
    }
    
    function strip(type: string | undefined) {
        return type?.replaceAll("<", "").replaceAll(">", "") ?? "";
    }
</script>
<div>
    <DefinitionEntry isSub={false} phrase={getPhrase(definition)} wordClass={definition.wordClass}/>
    {#each definition.inflections ?? [] as inflection}
        <DefinitionEntry isSub={true} phrase={inflection.word} wordClass="Verb ({strip(inflection.type)})"/>
    {/each}
</div>

<style>
    div {
        border: thin solid cornflowerblue;
        padding: 10px 20px;
        border-radius: 5px;
        margin-bottom: 10px;
    }
</style>

