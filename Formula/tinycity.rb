class Tinycity < Formula
  desc "Ask any large language model from your terminal via OpenAI-compatible APIs"
  homepage "https://github.com/yetanotherchris/tinycity"
  version "3.4.0"
  license "MIT"

  on_macos do
    if Hardware::CPU.arm?
      url "https://github.com/yetanotherchris/tinycity/releases/download/v3.4.0/tinycity-v3.4.0-osx-arm64"
      sha256 "396e22b100376b866051383c33c5c37b1b182575ed2259ea505b5e7ffd66e639"
    else
      url "https://github.com/yetanotherchris/tinycity/releases/download/v3.4.0/tinycity-v3.4.0-osx-x64"
      sha256 "5114a503c2062589ad80b83fcf7f3668741c503ab14e9002e966bc3306082a28"
    end
  end

  on_linux do
    url "https://github.com/yetanotherchris/tinycity/releases/download/v3.4.0/tinycity-v3.4.0-linux-x64"
    sha256 "79ca2a587cfdfc1cd85e8af61ed0ccbc5b94ee68d28b1677baab65a5bfc99dfc"
  end

  def install
    if OS.mac?
      if Hardware::CPU.arm?
        bin.install "tinycity-v3.4.0-osx-arm64" => "tinycity"
      else
        bin.install "tinycity-v3.4.0-osx-x64" => "tinycity"
      end
    else
      bin.install "tinycity-v3.4.0-linux-x64" => "tinycity"
    end
  end

  test do
    assert_match "USAGE:", shell_output("#{bin}/tinycity --help")
  end
end



















