class Tinycity < Formula
  desc "Ask any large language model from your terminal via OpenAI-compatible APIs"
  homepage "https://github.com/yetanotherchris/tinycity"
  version "2.0.4"
  license "MIT"

  on_macos do
    if Hardware::CPU.arm?
      url "https://github.com/yetanotherchris/tinycity/releases/download/v2.0.4/tinycity-v2.0.4-osx-arm64"
      sha256 "bcf673049452b1040b2d64f48d494a63981673653e59c9afab484dc407bd21fb"
    else
      url "https://github.com/yetanotherchris/tinycity/releases/download/v2.0.4/tinycity-v2.0.4-osx-x64"
      sha256 "9227fa19e19118c6b912455598723e5a8e5e462fa0528b3df1fe295916a3d3b5"
    end
  end

  on_linux do
    url "https://github.com/yetanotherchris/tinycity/releases/download/v2.0.4/tinycity-v2.0.4-linux-x64"
    sha256 "8ff19c4ec5b48328ce0ab5594187f1af88ea4eae88cfaccb2e9b443f9172ad85"
  end

  def install
    if OS.mac?
      if Hardware::CPU.arm?
        bin.install "tinycity-v2.0.4-osx-arm64" => "tinycity"
      else
        bin.install "tinycity-v2.0.4-osx-x64" => "tinycity"
      end
    else
      bin.install "tinycity-v2.0.4-linux-x64" => "tinycity"
    end
  end

  test do
    assert_match "USAGE:", shell_output("#{bin}/tinycity --help")
  end
end






