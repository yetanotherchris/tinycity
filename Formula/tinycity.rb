class Tinycity < Formula
  desc "Ask any large language model from your terminal via OpenAI-compatible APIs"
  homepage "https://github.com/yetanotherchris/tinycity"
  version "2.5.0"
  license "MIT"

  on_macos do
    if Hardware::CPU.arm?
      url "https://github.com/yetanotherchris/tinycity/releases/download/v2.5.0/tinycity-v2.5.0-osx-arm64"
      sha256 "750cbb24ed66f9e752c5c6e74920909f96bde445ea4f7d22a884920bfb24b995"
    else
      url "https://github.com/yetanotherchris/tinycity/releases/download/v2.5.0/tinycity-v2.5.0-osx-x64"
      sha256 "6e4ad99a500573e8b39a1b6cc04c6bdc1b9025121e2364daaf075b18c794111e"
    end
  end

  on_linux do
    url "https://github.com/yetanotherchris/tinycity/releases/download/v2.5.0/tinycity-v2.5.0-linux-x64"
    sha256 "724e09a29a8d1147ab31d3a41247143cf146e934578500d0ff5bd29f2e626bdc"
  end

  def install
    if OS.mac?
      if Hardware::CPU.arm?
        bin.install "tinycity-v2.5.0-osx-arm64" => "tinycity"
      else
        bin.install "tinycity-v2.5.0-osx-x64" => "tinycity"
      end
    else
      bin.install "tinycity-v2.5.0-linux-x64" => "tinycity"
    end
  end

  test do
    assert_match "USAGE:", shell_output("#{bin}/tinycity --help")
  end
end













