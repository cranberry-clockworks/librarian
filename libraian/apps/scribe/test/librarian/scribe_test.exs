defmodule Librarian.ScribeTest do
  use ExUnit.Case
  doctest Librarian.Scribe

  test "greets the world" do
    assert Librarian.Scribe.hello() == :world
  end
end
