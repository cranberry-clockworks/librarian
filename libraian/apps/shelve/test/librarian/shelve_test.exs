defmodule Librarian.ShelveTest do
  use ExUnit.Case
  doctest Librarian.Shelve

  test "greets the world" do
    assert Librarian.Shelve.hello() == :world
  end
end
