class Tinycity < Formula
  desc "Ask any large language model from your terminal via OpenAI-compatible APIs"
  homepage "https://github.com/yetanotherchris/tinycity"
  version "3.4.2"
  license "MIT"

  on_macos do
    if Hardware::CPU.arm?
      url "https://github.com/yetanotherchris/tinycity/releases/download/v3.4.2/tinycity-v3.4.2-osx-arm64"
      sha256 "8aa06f3700762a04580c8b356ea52b1463b85e2a61e867aa521a5b5c8ef16395"
    else
      url "https://github.com/yetanotherchris/tinycity/releases/download/v3.4.2/tinycity-v3.4.2-osx-x64"
      sha256 "ad6d929393d8f6a6a115773df7ff2880a610d0ef33927e441861a2299b0cf224"
    end
  end

  on_linux do
    url "https://github.com/yetanotherchris/tinycity/releases/download/v3.4.2/tinycity-v3.4.2-linux-x64"
    sha256 "3be93571f9a1e9900795c1d33b8610a79b8c5e26ffd06f851697603f60a0a29c"
  end

  def install
    if OS.mac?
      if Hardware::CPU.arm?
        bin.install "tinycity-v3.4.2-osx-arm64" => "tinycity"
      else
        bin.install "tinycity-v3.4.2-osx-x64" => "tinycity"
      end
    else
      bin.install "tinycity-v3.4.2-linux-x64" => "tinycity"
    end
  end

  test do
    assert_match "USAGE:", shell_output("#{bin}/tinycity --help")
  end
end





















