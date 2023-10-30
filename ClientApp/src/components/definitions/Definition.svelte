<script lang="ts">
    import type {Noun, Phrase, Verb} from "$lib/generated/client";
    import {WordClass} from "$lib/definition";
    
    import VerbDefinitionBody from "./VerbDefinitionBody.svelte";
    import DefinitionHeader from "./DefinitionHeader.svelte";
    
    export let definition: Noun | Verb | Phrase
    
    function getPhraseForNoun(definition: Noun) {
        return `${definition.article} ${definition.lemma}`;
    }
</script>
<div>
    {#if definition.$type === WordClass.Noun}
        <DefinitionHeader phrase={getPhraseForNoun(definition)} wordClass="Noun"/>
    {:else if definition.$type === WordClass.Verb}
        <DefinitionHeader phrase="{definition.lemma}" wordClass="Verb" >
            <VerbDefinitionBody definition={definition}/>
        </DefinitionHeader>
    {:else }
        <DefinitionHeader phrase="{definition.lemma}" wordClass="{definition.$type}"/>
    {/if}
</div>

<style>
    div {
        border: thin solid cornflowerblue;
        padding: 10px 20px;
        border-radius: 5px;
        margin-bottom: 10px;
    }
</style>

