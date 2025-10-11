class Tinycity < Formula
  desc "Ask any large language model from your terminal via OpenAI-compatible APIs"
  homepage "https://github.com/yetanotherchris/tinycity"
  version "2.2.0"
  license "MIT"

  on_macos do
    if Hardware::CPU.arm?
      url "https://github.com/yetanotherchris/tinycity/releases/download/v2.2.0/tinycity-v2.2.0-osx-arm64"
      sha256 "9bc8a8a3c30e1a929fe2ecb3acc053544c231054258b621df07fb66d8d3295a2"
    else
      url "https://github.com/yetanotherchris/tinycity/releases/download/v2.2.0/tinycity-v2.2.0-osx-x64"
      sha256 "e0f8ea0bf5abf4ca1062b202c6a1fc40800da77a695a702978c3e2d50e9fbefe"
    end
  end

  on_linux do
    url "https://github.com/yetanotherchris/tinycity/releases/download/v2.2.0/tinycity-v2.2.0-linux-x64"
    sha256 "be2e8340b40216e7560a6a80f9cbacba3eb6f2bb5bf3642e37066f8ea243856e"
  end

  def install
    if OS.mac?
      if Hardware::CPU.arm?
        bin.install "tinycity-v2.2.0-osx-arm64" => "tinycity"
      else
        bin.install "tinycity-v2.2.0-osx-x64" => "tinycity"
      end
    else
      bin.install "tinycity-v2.2.0-linux-x64" => "tinycity"
    end
  end

  test do
    assert_match "USAGE:", shell_output("#{bin}/tinycity --help")
  end
end









